using Conversa.Runtime;

namespace Conversa.Editor
{
    public static class BlockNodeSearchMenuModifier
    {
        [StackNodeMenuModifier]
        private static void ModifyMenu(NodeMenuTree tree, Conversation conversation)
        {
            tree.AddMenuEntry<MessageBlockNodeView>("Message", 1);
        }
    }
}