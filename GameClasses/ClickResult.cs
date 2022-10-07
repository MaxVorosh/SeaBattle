namespace GameClasses;

public enum ClickResult
{
    /// <summary>
    /// Variations of what was happened when we click on the board in prepare to the game window
    /// Delete - One ship was deletes
    /// Put - One ship was added
    /// Sleep - Nothing 
    /// </summary>
    Delete, 
    Put,
    Sleep
}