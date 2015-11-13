#pragma once
using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Xaml::Media::Imaging;

namespace BackgroundTasks
{
	public ref class TileUpdaterTask sealed : public XamlRenderingBackgroundTask
	{

	public:
		TileUpdaterTask();

		virtual void Run(IBackgroundTaskInstance^ taskInstance);
		void OnCompleted(
			BackgroundTaskRegistration^ task,
			BackgroundTaskCompletedEventArgs^ args
			);
	};
}
