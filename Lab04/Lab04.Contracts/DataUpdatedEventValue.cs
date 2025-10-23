namespace Lab04.Contracts;

public record DataUpdatedEventValue(string Data);
public class DataUpdatedEvent : PubSubEvent<DataUpdatedEventValue>;
