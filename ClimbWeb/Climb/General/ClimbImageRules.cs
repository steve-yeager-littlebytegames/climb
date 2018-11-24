namespace Climb
{
    public static class ClimbImageRules
    {
        public const long ProfilePicMaxSize = 1024 * 100;
        public const long CharacterImageMaxSize = 100 * 1024;
        public const long GameLogoMaxSize = 100 * 1024;

        public static ImageRules ProfilePic { get; } = new ImageRules(ProfilePicMaxSize, "profile-pics", string.Empty);
        public static ImageRules CharacterPic { get; } = new ImageRules(CharacterImageMaxSize, "character-pics", string.Empty);
        public static ImageRules GameLogo { get; } = new ImageRules(GameLogoMaxSize, "game-logo", string.Empty);
    }
}