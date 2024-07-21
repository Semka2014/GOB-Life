# GOB-Life

![Simulation Image](https://i.imgur.com/tKslvqX.png)

## Описание

GOB-Life — это симуляция естественного отбора, в которой боты-клетки эволюционируют на игровом поле. У каждой клетки есть ДНК и мозг, который формируется на основе этой ДНК при её рождении. ДНК может мутировать, что позволяет клеткам адаптироваться и развиваться.

Мозг клетки состоит из блоков:

- **Блоки-действия**: отвечают за выполнение определенных действий.
- **Блоки-датчики**: воспринимают информацию из окружающей среды.
- **Блоки-обработчики**: соединяют блоки-действия с блоками-датчиками и обрабатывают сигналы.

Геном клетки представлен последовательностями кодонов, которые можно разделить на гены. Каждый ген функционирует как независимый органоид, выполняющий определенные функции.

## Половое размножение

Боты в этой симуляции размножаются с помощью полового размножения. Но разделения на полы, как такого нет. 
Есть родитель 1 - инициатор размножения и родитель 2 - сосед первого родителя. Согласия второго родителя не требуется. 
Потомку передаются гены обоих родителей, каждый с вероятностью 50 на 50.
Возможно, в дальнейшем будут использоваться другие, более сложные подходы.