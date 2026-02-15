namespace EC.Services
{
    public abstract class GameService
    {
        /// <summary>
        /// Override this method to perform initialization logic when the service is registered. This method is called by the GameLocator when the service is registered, and it is guaranteed to be called on the main thread. You can use this method to set up any necessary resources or state for your service.
        /// </summary>
        public virtual void OnCreate()
        {

        }
        /// <summary>
        /// Override this method to perform cleanup logic when the service is unregistered. This method is called by the GameLocator when the service is unregistered, and it is guaranteed to be called on the main thread. You can use this method to release any resources or state that your service holds.
        /// </summary>
        public virtual void OnDispose()
        {
            
        }
    }
}
