namespace Climb
{
    public static class ClimbImageRules
    {
        public const long ProfilePicMaxSize = 1024 * 100;
        public const long CharacterImageMaxSize = 100 * 1024;

        public static ImageRules ProfilePic { get; } = new ImageRules(ProfilePicMaxSize, 150, 150, "profile-pics", string.Empty);
        public static ImageRules CharacterPic { get; } = new ImageRules(CharacterImageMaxSize, 150, 150, "character-pics", string.Empty);
        public static ImageRules GameLogo { get; } = new ImageRules(100 * 1024, 150, 150, "game-logo", string.Empty);
    }
}