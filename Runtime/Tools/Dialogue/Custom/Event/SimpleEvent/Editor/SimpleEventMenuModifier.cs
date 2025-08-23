using Conversa.Editor;
using Conversa.Runtime;

public class SimpleEventNodeMenuModifier
{
    [NodeMenuModifier]
    private static void ModifyMenu(NodeMenuTree tree, Conversation conversation)
    {
        tree.AddMenuEntry<SimpleEventNodeView>("SimpleEvent", 1);
    }
}