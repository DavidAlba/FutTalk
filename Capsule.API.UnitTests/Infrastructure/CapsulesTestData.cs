using Capsule.API.Models;
using System.Collections;
using System.Collections.Generic;

namespace Capsule.API.UnitTests
{
    public class CapsulesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { GetCapsules() };
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private IEnumerable<object> GetCapsules()
        {
            for (int i = 0; i < 5; i++)
            {
                var capsule = new Models.Capsule($"User {i + 1}");

                var messages = new List<Message>();
                for (int j = 0; j < 5; j++)
                {
                    messages.Add(
                        new Message
                        {
                            Id = j + 1,
                            Name = $"Name {j + 1}",
                            Body = $"Body {j + 1}"
                        });
                }

                capsule.Messages = messages;
                yield return capsule;
            }
        }
    }
}
