#include "pch.h"
#include <ppl.h>
#include <sstream>
#include <iostream>
#include <vector>
#include "TileUpdaterTask.h"

#pragma once
using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Web::Http;
using namespace Windows::Storage;
using namespace Platform;
using namespace concurrency;
using namespace Windows::Data::Json;
using namespace std;
using namespace BackgroundTasks;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Markup;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Foundation;
using namespace Windows::Storage::Streams;
using namespace Windows::Graphics::Imaging;

TileUpdaterTask::TileUpdaterTask()
{

}

void TileUpdaterTask::Run(IBackgroundTaskInstance^ taskInstance)
{
	Agile<BackgroundTaskDeferral^> deferral = Agile<BackgroundTaskDeferral^>(taskInstance->GetDeferral());

	create_task(GetScheduelsAsync())
		.then([=]()
	{
		if (m_listSize > 0)
		{
			create_task(GetUIElementToRenderAsync())
				.then([=](Grid^ grid)
			{
				create_task(RenderAndSaveToFileAsync(grid, "mTile.png", 150, 150))
					.then([=]()
				{
					UpdateTile("mTile.png");
					deferral->Complete();
				});
			});
		}
	});
}

void TileUpdaterTask::OnCompleted(BackgroundTaskRegistration ^ task, BackgroundTaskCompletedEventArgs ^ args)
{

}

IAsyncOperation<Grid^>^ TileUpdaterTask::GetUIElementToRenderAsync()
{
	return create_async([=]()
	{
		Grid^ grid = nullptr;
		Grid^* gridp = &grid;
		create_task(StorageFile::GetFileFromApplicationUriAsync(ref new Uri("/Assets/TileForBgTask.xml")))
			.then([=](StorageFile^ file)
		{
			create_task(FileIO::ReadTextAsync(file))
				.then([=](Platform::String^ markupContent)
			{
				Grid^ rootGrid = (Grid^)XamlReader::Load(markupContent);
				*gridp = (Grid^)rootGrid->FindName(L"MiddleGrid");
			});
		}).wait();
		return *gridp;
	});
}

std::vector<std::wstring> TileUpdaterTask::SplitString(const std::wstring & String, const std::wstring & Seperator)
{
	std::vector<std::wstring> Lines;
	size_t stSearchPos = 0;
	size_t stFoundPos;
	while (stSearchPos < String.size() - 1)
	{
		stFoundPos = String.find(Seperator, stSearchPos);
		stFoundPos = (stFoundPos == std::string::npos) ? String.size() : stFoundPos;
		Lines.push_back(String.substr(stSearchPos, stFoundPos - stSearchPos));
		stSearchPos = stFoundPos + Seperator.size();
	}
	return Lines;
}


task<void> TileUpdaterTask::GetScheduelsAsync()
{
	auto container = ApplicationData::Current->LocalSettings;

	if (!container->Values->HasKey("sid") || !container->Values->HasKey("access_token"))
	{
		return create_task([=]()
		{
			return;
		});
	}
	String^ sid = (String^)container->Values->Lookup("sid");
	String^ token = (String^)container->Values->Lookup("access_token");

	String^ postStr = nullptr;
	if (sid != nullptr && token != nullptr)
	{
		postStr = "sid=" + sid + "&access_token=" + token;
	}

	if (postStr == nullptr)
	{
		return create_task([=]()
		{
			return;
		});
	}

	HttpClient^ client = ref new HttpClient();
	HttpStringContent^ content = ref new HttpStringContent(postStr);

	return create_task(client->PostAsync(ref new Windows::Foundation::Uri(L"http://juniperphoton.net/schedule/Schedule/GetMySchedules/v2?"), content))
		.then([=](HttpResponseMessage^ resp) -> IAsyncOperationWithProgress<String^, unsigned long long>^ {
		IHttpContent^ content = resp->Content;
		return content->ReadAsStringAsync();
	}).then([=](String^ contentString)
	{
		JsonObject^* jsonObject = nullptr;
		if (!JsonObject::TryParse(contentString, jsonObject))
		{
			return;
		}
		auto ok = (*jsonObject)->GetNamedBoolean(L"isSuccessed");
		if (!ok)
		{
			return;
		}
		auto listarray = (*jsonObject)->GetNamedArray(L"ScheduleInfo");
		auto orderObject = (*jsonObject)->GetNamedArray(L"OrderList")->GetObjectAt(0);
		auto orderStr = orderObject->GetNamedString("list_order");
		std::wstring str = orderStr->Data();

		auto orderVector = SplitString(str, L",");

		auto rawList = new ToDo[listarray->Size];

		int undoneCount = 0;
		for (int i = 0; i < listarray->Size; i++)
		{
			auto todoObj = listarray->GetObjectAt(i);
			auto content = todoObj->GetNamedString("content");
			auto done = todoObj->GetNamedString("isdone");
			auto id = todoObj->GetNamedString("id");
			if (done == "0")
			{
				undoneCount++;
			}
			ToDo todo;
			todo.content = content;
			todo.done = done == "0" ? false : true;
			todo.id = id;
			rawList[i] = todo;
		}

		m_list = new ToDo[orderVector.size()];

		for (int i = 0; i < orderVector.size(); i++)
		{
			auto id = orderVector.at(0);
			for (int i = 0; i < listarray->Size; i++)
			{
				ToDo todo = rawList[i];
				if (todo.id->Data() == id)
				{
					m_list[i] = todo;
				}
			}
		}

		m_listSize = orderVector.size();
	});
}

task<void> TileUpdaterTask::RenderAndSaveToFileAsync(UIElement^ uiElement, String^ outputImageFilename, uint32 width, uint32 height)
{
	RenderTargetBitmap^ rtb = ref new RenderTargetBitmap();
	return create_task(rtb->RenderAsync(uiElement, width, height))
		.then([this, rtb]() -> IAsyncOperation<IBuffer^>^ {
		this->pixelWidth = (uint32)rtb->PixelWidth;
		this->pixelHeight = (uint32)rtb->PixelHeight;
		return rtb->GetPixelsAsync();
	}).then([this, rtb, outputImageFilename](IBuffer^ buffer) {
		StorePixelsFromBuffer(buffer);
		return WriteBufferToFile(outputImageFilename);
	});
}

Array<unsigned char>^ TileUpdaterTask::GetArrayFromBuffer(IBuffer^ buffer)
{
	Streams::DataReader^ dataReader = Streams::DataReader::FromBuffer(buffer);
	Array<unsigned char>^ data = ref new Array<unsigned char>(buffer->Length);
	dataReader->ReadBytes(data);
	return data;
}

void TileUpdaterTask::StorePixelsFromBuffer(IBuffer^ buffer)
{
	this->pixelData = GetArrayFromBuffer(buffer);
}

task<void> TileUpdaterTask::WriteBufferToFile(String^ outputImageFilename)
{
	auto resultStorageFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;

	return create_task(resultStorageFolder->CreateFileAsync(outputImageFilename, CreationCollisionOption::ReplaceExisting)).
		then([](StorageFile^ outputStorageFile) ->IAsyncOperation<IRandomAccessStream^>^ {
		return outputStorageFile->OpenAsync(FileAccessMode::ReadWrite);
	}).then([](IRandomAccessStream^ outputFileStream) ->IAsyncOperation<BitmapEncoder^>^ {
		return BitmapEncoder::CreateAsync(BitmapEncoder::PngEncoderId, outputFileStream);
	}).then([this](BitmapEncoder^ encoder)->IAsyncAction^ {
		encoder->SetPixelData(BitmapPixelFormat::Bgra8, BitmapAlphaMode::Premultiplied, this->pixelWidth, this->pixelHeight, 96, 96, this->pixelData);
		return encoder->FlushAsync();
	}).then([this]() {
		this->pixelData = nullptr;
		return;
	});
}

// Send a tile notification with the new tile payload. 
void TileUpdaterTask::UpdateTile(String^ tileUpdateImagePath)
{
	auto tileUpdater = TileUpdateManager::CreateTileUpdaterForApplication();
	tileUpdater->Clear();
	auto tileTemplate = TileUpdateManager::GetTemplateContent(TileTemplateType::TileSquare150x150Image);
	auto tileImageAttributes = tileTemplate->GetElementsByTagName("image");
	static_cast<XmlElement^>(tileImageAttributes->Item(0))->SetAttribute("src", tileUpdateImagePath);
	auto notification = ref new TileNotification(tileTemplate);
	tileUpdater->Update(notification);
}
