namespace CommandLineParser
{
    internal static class CommandReferencer
    {
        internal static List<CommandReferenceBag> commandReferences = new();

        internal static CommandReferenceBag AddCommandReference<T>(T command)
        {
            CommandReferenceBag commandReferenceBag = new(new WeakReference(command), typeof(T));

            commandReferences.Add(commandReferenceBag);

            return commandReferenceBag;
        }

        internal static void CleanUnusedCommandReferences()
        {
            for (int i = commandReferences.Count - 1; i >= 0; i--)
            {
                if (!commandReferences[i].Reference.IsAlive) commandReferences.RemoveAt(i);
            }
        }
    }
}