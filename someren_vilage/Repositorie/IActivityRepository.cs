using System.Collections.Generic;
using someren_vilage.Models;

namespace someren_vilage.Repositorie
{
    public interface IActivityRepository
    {
        List<Activity> GetAll();
        Activity? GetById(int activityId);
        void Add(Activity activity);
        void Update(Activity activity);
        void Delete(int activityId);
    }
}
