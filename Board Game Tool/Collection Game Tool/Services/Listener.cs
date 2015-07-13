namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// The listener/audience
	/// </summary>
    public interface Listener
    {
		/// <summary>
		/// Called when shouted
		/// </summary>
		/// <param name="pass">The object that was passed</param>
        void OnListen(object pass);
    }
}
