namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// The teller/shouter
	/// </summary>
    public interface Teller
    {
		/// <summary>
		/// Shouts to the listeners/audience.
		/// </summary>
		/// <param name="pass">The object to pass</param>
        void Shout(object pass);
		/// <summary>
		/// Adds a listener
		/// </summary>
		/// <param name="listener">The listener to add</param>
        void AddListener(Listener listener);
    }
}
