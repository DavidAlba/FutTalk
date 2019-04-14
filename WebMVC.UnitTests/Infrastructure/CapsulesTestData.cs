using System.Collections;
using System.Collections.Generic;
using WebMVC.Models;

namespace WebMVC.UnitTests
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
            List<Capsule> capsules = new List<Capsule>();
            for (int i = 0; i < 5; i++)
            {
                var capsule = new Capsule($"User {i + 1}");
                for (int j = 0; j < 5; j++)
                    capsule.AddMessage(new Message { Id = j, Name = $"Name {j}", Body = "Body {j}" });
                capsules.Add(capsule);
            }

            return capsules;
        }
    }
}
