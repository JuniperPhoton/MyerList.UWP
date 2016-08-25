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

		ToDo* m_list;
		bool GetScheduels();
		void UpdateTile();
		void RenderAndSave(Windows::UI::Xaml::UIElement^ element);

		std::vector<std::wstring> SplitString(const std::wstring & String, const std::wstring & Seperator);
	};
}
