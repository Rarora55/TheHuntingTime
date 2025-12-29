public interface IStaminaConsumer
{
    bool ConsumeStamina(float amount);
    bool CanConsumeStamina(float amount);
}
