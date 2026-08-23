namespace SaveLoadModule.DocumentDto
{
    /// <summary>
    /// A gameplay system that owns one section of <see cref="SaveGameDocument"/>.
    /// </summary>
    public interface ISaveParticipant
    {
        string SectionId { get; }

        string CaptureJson();

        void ClearRuntime();

        void RestoreJson(string json);
    }
}
