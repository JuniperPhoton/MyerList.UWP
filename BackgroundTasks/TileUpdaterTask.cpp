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

TileUpdaterTask::TileUpdaterTask()
{

}

void TileUpdaterTask::Run(IBackgroundTaskInstance^ taskInstance)
{

}

void TileUpdaterTask::OnCompleted(BackgroundTaskRegistration ^ task, BackgroundTaskCompletedEventArgs ^ args)
{

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


bool TileUpdaterTask::GetScheduels()
{
	auto container = ApplicationData::Current->LocalSettings;

	if (!container->Values->HasKey("sid") || !container->Values->HasKey("access_token"))
	{
		return false;
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
		return false;
	}

	HttpClient^ client = ref new HttpClient();
	HttpStringContent^ content = ref new HttpStringContent(postStr);
	create_task(client->PostAsync(ref new Windows::Foundation::Uri(L"http://juniperphoton.net/schedule/Schedule/GetMySchedules/v2?"), content))
		.then([=](HttpResponseMessage^ resp)
	{
		if (resp->IsSuccessStatusCode)
		{
			return;
		}
		IHttpContent^ content = resp->Content;
		create_task(content->ReadAsStringAsync())
			.then([=](String^ contentString)
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
		});
	});
}

void TileUpdaterTask::UpdateTile()
{

}

void TileUpdaterTask::RenderAndSave(Windows::UI::Xaml::UIElement^ element)
{

}
