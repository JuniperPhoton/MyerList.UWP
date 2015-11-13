#include "TileUpdaterTask.h"
#include "pch.h"
#pragma once
using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Xaml::Media::Imaging;

namespace BackgroundTasks
{
	public ref class TileUpdaterTask sealed : public XamlRenderingBackgroundTask
	{

	public:
		TileUpdaterTask()
		{

		}

		void Run(IBackgroundTaskInstance^ taskInstance)
		{

		}
		void TileUpdaterTask::OnCompleted(BackgroundTaskRegistration ^ task, BackgroundTaskCompletedEventArgs ^ args)
		{
			
		}
	};
}
