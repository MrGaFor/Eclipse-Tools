using Conversa.Editor;
using Conversa.Runtime;

public class SimpleChoiceNodeMenuModifier
{
    [NodeMenuModifier]
    private static void ModifyMenu(NodeMenuTree tree, Conversation conversation)
    {
        tree.AddMenuEntry<SimpleChoiceNodeView>("SimpleChoice", 1);
    }
}