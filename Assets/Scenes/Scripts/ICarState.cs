using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana
public interface ICarState
{

    /// <summary>
    /// Te trzy metody bêdzie musia³ posiadaæ ka¿dy stan naszego auta
    /// </summary>


    /* 
     Metoda Enter - jest wywo³ywana, gdy samochód wchodzi w dany stan.
     Przyjmuje jako argument obiekt CarController, który reprezentuje kontroler
     samochodu i umo¿liwia dostêp do jego w³aœciwoœci i metod.
     Ta metoda jest odpowiedzialna za inicjalizacjê stanu, ustawienie odpowiednich
     parametrów i przygotowanie samochodu do dzia³ania w tym stanie. 
    */

    void Enter(CarController car);


    /* 
     Metoda Exit - jest wywo³ywana, gdy samochód opuszcza dany stan. Przyjmuje jako 
     argument obiekt CarController, który reprezentuje kontroler samochodu i umo¿liwia 
     dostêp do jego w³aœciwoœci i metod. Ta metoda jest odpowiedzialna za czyszczenie 
     stanu, resetowanie parametrów i przygotowanie samochodu do przejœcia do innego stanu.
    */

    void UpdateState(CarController car);


    /* 
     Metoda UpdateState - jest wywo³ywana w ka¿dej klatce, gdy samochód znajduje 
     siê w danym stanie. Przyjmuje jako argument obiekt CarController, który 
     reprezentuje kontroler samochodu i umo¿liwia dostêp do jego w³aœciwoœci i metod. 
     Ta metoda jest odpowiedzialna za aktualizacjê stanu, wykonywanie logiki specyficznej 
     dla tego stanu oraz podejmowanie decyzji dotycz¹cych przejœcia do innych 
     stanów na podstawie warunków i sytuacji na drodze.
    */

    void Exit(CarController car);
}
