using Conversa.Editor;
using Conversa.Runtime;

public class SimpleMassiveNodeMenuModifier
{
    [NodeMenuModifier]
    private static void ModifyMenu(NodeMenuTree tree, Conversation conversation)
    {
        tree.AddMenuEntry<SimpleMassiveNodeView>("SimpleMassive", 1);
    }
}