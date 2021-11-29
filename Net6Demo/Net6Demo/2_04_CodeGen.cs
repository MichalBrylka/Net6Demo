using Microsoft.Extensions.Logging;

namespace Net6Demo
{
    internal static partial class CodeGen
    {
        public partial class TestController
        {
            //Error SYSLIB1019  Couldn't find a field of type Microsoft.Extensions.Logging.ILogger in class TestController	
            private readonly ILogger<TestController> _logger;
            public TestController(ILogger<TestController> logger) => _logger = logger;

            [LoggerMessage(0, LogLevel.Information, "Writing hello world response to {Person}",
                EventName = "HelloWorldEvent")]
            partial void LogHelloWorld(Person person);

            //Error CS0103  The name 'Reason' does not exist in the current context Net6Demo 
            //[LoggerMessage(0, LogLevel.Information, "Writing hello world response to {Person} with a {Reason}")]
            //partial void LogHelloWorld2(Person person);

            internal void ProcessPerson(Person p) => LogHelloWorld(p);
        }

        internal record Person(string Name, int Age, bool IsAlive);

        public static void TestLogging()
        {
            /*var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddFilter("Net6Demo", LogLevel.Warning)
                        .AddConsole();
                }
            );
            var controller = new TestController(loggerFactory.CreateLogger<TestController>());*/

            var simpleLogger = new SimpleLogger<TestController>();
            var controller = new TestController(simpleLogger);

            controller.ProcessPerson(new("Mike", 38, true));

            var text = simpleLogger.ToString();
        }

        public static void TestEventSourceGen()
        {      
            
        }

        internal enum AnimalKind { Mammal, Bird, Reptile };
        internal enum AnimalFood { Carnivorous, Herbivorous, Omnivorous };

        internal class Animal
        {
            public string? Name { get; set; }
            public AnimalKind Kind { get; set; }
            public AnimalFood Food { get; set; }
        }

        [JsonSerializable(typeof(Animal))]
        [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
        internal partial class MyJsonContext : JsonSerializerContext { }

        public static void TestJsonGen()
        {
            var animal = new Animal() { Name = "Lion", Food = AnimalFood.Carnivorous, Kind = AnimalKind.Mammal };
            byte[] utf8Json = JsonSerializer.SerializeToUtf8Bytes(animal, MyJsonContext.Default.Animal);
            var newAnimal = JsonSerializer.Deserialize(utf8Json, MyJsonContext.Default.Animal);
        }
    }

    internal class SimpleLogger<T> : ILogger<T>
    {
        private readonly StringBuilder _sb = new();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            _sb.AppendLine($"{logLevel}: {eventId} {formatter(state, exception)}");
        }

        public override string ToString() => _sb.ToString();

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
    }
}
