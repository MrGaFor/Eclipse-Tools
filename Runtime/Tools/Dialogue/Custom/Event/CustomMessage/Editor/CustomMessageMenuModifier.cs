using Conversa.Editor;
using Conversa.Runtime;

public class CustomMessageNodeMenuModifier
{
    [NodeMenuModifier]
    private static void ModifyMenu(NodeMenuTree tree, Conversation conversation)
    {
        tree.AddMenuEntry<CustomMessageNodeView>("CustomMessage", 1);
    }
}