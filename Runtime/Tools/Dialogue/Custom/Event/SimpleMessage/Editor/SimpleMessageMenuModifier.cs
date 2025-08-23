using Conversa.Editor;
using Conversa.Runtime;

public class SimpleMessageNodeMenuModifier
{
    [NodeMenuModifier]
    private static void ModifyMenu(NodeMenuTree tree, Conversation conversation)
    {
        tree.AddMenuEntry<SimpleMessageNodeView>("SimpleMessage", 1);
    }
}