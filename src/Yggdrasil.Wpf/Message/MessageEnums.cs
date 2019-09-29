namespace Yggdrasil.Wpf
{
    /// <summary>
    /// Represents a result for a message.
    /// </summary>
    public enum MessageResult
    {
        None,
        Yes,
        No,
        Ok,
        Cancel
    }

    /// <summary>
    /// Represents options for a message.
    /// </summary>
    public enum MessageOptions
    {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel
    }

    /// <summary>
    /// Represents option for the image of a message.
    /// </summary>
    public enum MessageImage
    {
        Information,
        Error,
        Question,
        Warning
    }
}
