using FluentAssertions;
using Challenge02.Infraestructure.Serializer;
using Challenge02.Infraestructure.UnitTests.Common;

namespace Challenge02.Infraestructure.UnitTests.Serializer
{
    public class CustomSerializerTests
    {
        [Fact]
        public void Serialize_WhenValidObject_ReturnsJsonString()
        {
            var serializer = new CustomSerializer();
            var testObject = new Foo { Field1 = "Test", Field2 = 123 };

            var jsonString = serializer.Serialize(testObject);

            jsonString.Should().Be("{\"Field1\":\"Test\",\"Field2\":123}");
        }

        [Fact]
        public void Deserialize_WhenValidJsonString_ReturnsObject()
        {
            var serializer = new CustomSerializer();
            var jsonString = "{\"Field1\":\"Test\",\"Field2\":123}";
            var expectedType = typeof(Foo);

            var result = serializer.Deserialize(jsonString, expectedType);

            result.Should().BeOfType<Foo>();
            var testObject = result as Foo;
            testObject.Field1.Should().Be("Test");
            testObject.Field2.Should().Be(123);
        }
    }
}
