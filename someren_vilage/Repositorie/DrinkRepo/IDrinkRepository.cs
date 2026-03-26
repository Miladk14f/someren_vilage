using someren_vilage.Models;

namespace someren_vilage.Repositorie.DrinkRepo
{
    public interface IDrinkRepository
    {
        List<Drink> GetAll();
        Drink? GetById(int drinkId);
        void Add(Drink drink);
        void Update(Drink drink);
        void Delete(int drinkId);
    }
}
