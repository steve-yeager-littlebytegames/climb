namespace Climb
{
    public static class ClimbImageRules
    {
        public static ImageRules ProfilePic { get; } = new ImageRules(100 * 1024, 150, 150, "profile-pics", string.Empty);
        public static ImageRules CharacterPic { get; } = new ImageRules(100 * 1024, 150, 150, "character-pics", string.Empty);
        public static ImageRules GameLogo { get; } = new ImageRules(100 * 1024, 150, 150, "game-logo", string.Empty);
    }
}