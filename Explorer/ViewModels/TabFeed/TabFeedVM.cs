using Explorer.Interfaces.MessageBus;
using Explorer.Interfaces;

namespace Explorer.ViewModels.TabBar;

public class TabFeedVM
{
    public IndexFeedVM IndexFeedVM { get; init; }
    public VisionFeedVM VisionFeedVM { get; init; }

    public TabFeedVM(IMessageBus messageBus)
    {
        IndexFeedVM = new IndexFeedVM(messageBus);
        VisionFeedVM = new VisionFeedVM(messageBus);
    }
}
