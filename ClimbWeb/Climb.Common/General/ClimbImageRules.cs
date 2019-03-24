namespace Climb
{
    public static class ClimbImageRules
    {
        public const long ProfilePicMaxSize = 1024 * 100;
        public const long CharacterImageMaxSize = 100 * 1024;
        public const long GameLogoMaxSize = 100 * 1024;
        public const long GameBannerMaxSize = 5 * 1024 * 1024;

        public static ImageRules ProfilePic { get; } = new ImageRules(ProfilePicMaxSize, 150, 150, "profile-pics", string.Empty);
        public static ImageRules CharacterPic { get; } = new ImageRules(CharacterImageMaxSize, 150, 150, "character-pics", "/images/NewCharacterIcon.png");
        public static ImageRules GameLogo { get; } = new ImageRules(GameLogoMaxSize, 150, 150, "game-logo", string.Empty);
        public static ImageRules GameBanner { get; } = new ImageRules(GameBannerMaxSize, 2000, 500, "game-banner", string.Empty);
    }
}