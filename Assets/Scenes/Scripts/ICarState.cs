using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana
public interface ICarState
{
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
}
