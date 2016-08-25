#pragma once
using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Xaml::Media::Imaging;

namespace BackgroundTasks
{
	public ref class TileUpdaterTask sealed : public XamlRenderingBackgroundTask
	{
	public:
		TileUpdaterTask();
		void Run(IBackgroundTaskInstance^ taskInstance);
		void OnCompleted(BackgroundTaskRegistration^ task, BackgroundTaskCompletedEventArgs^ args);
	private:
		struct ToDo
		{
			Platform::String^ content;
			bool done;
			Platform::String^ id;
		};

		UINT m_listSize;
		ToDo* m_list;
		Windows::Foundation::IAsyncAction^ GetScheduelsAsync();

		Windows::Foundation::IAsyncOperation<Windows::UI::Xaml::Controls::Grid^>^ GetUIElementToRenderAsync();

		// Helper methods
		concurrency::task<void> WriteBufferToFile(Platform::String^ outputImageFilename);
		Platform::Array<unsigned char>^ GetArrayFromBuffer(Windows::Storage::Streams::IBuffer^ buffer);
		void StorePixelsFromBuffer(Windows::Storage::Streams::IBuffer^ buffer);

		// RenderTargetBitmap
		concurrency::task<void> RenderAndSaveToFileAsync(Windows::UI::Xaml::UIElement^ uiElement, Platform::String^ outputImageFilename, UINT width , UINT height);

		// RenderTargetBitmap Pixel Data
		unsigned int pixelWidth;
		unsigned int pixelHeight;
		Platform::Array<unsigned char>^ pixelData;

		// Tile Updating
		void UpdateTile(Platform::String^ tileUpdateImagePath);

		std::vector<std::wstring> SplitString(const std::wstring & String, const std::wstring & Seperator);
	};
}
