using Explorer.Interfaces.MessageBus;

namespace Explorer.ViewModels.TabBar;

public class VisionFeedVM
{
    private readonly IMessageBus _messageBus;

    public VisionFeedVM(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }
}
