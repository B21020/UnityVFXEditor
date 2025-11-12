namespace UnityVFXEditor.Effects
{
    public interface IEffect
    {
        void Initialize();
        void Trigger(float time);
    }
}
