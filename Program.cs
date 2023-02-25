using System.Data;

namespace CongratsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n \t\t\t\t\t-----Программа \"Поздравлятор\"-----");
            Console.WriteLine($"\t\t\t\t\t\tСегодня {DateTime.Today.ToLongDateString()}\n");
            using (AppContext db = new())
            {
                var birthdays = db.Birthdays.ToList();
                int birthdaysToday = 0;
                int yearNow = DateTime.Today.Year;
                var unsortedList = new List<SortBirthdays>();

                Console.WriteLine($"***У кого сегодня день рождения?***");

                foreach (var b in birthdays) //для каждого ДР из базы данных добавить оставшееся количество дней, и год, в котором он будет отмечаться 
                {
                    DateTime currentBirthday;
                    int birthYear;

                    if (b.Date.DayOfYear == DateTime.Today.DayOfYear) //если ДР сегодня
                    {
                        Console.WriteLine($"\t{birthdaysToday + 1}) {b.Name} – {yearNow - b.Date.Year} {RusWord(yearNow - b.Date.Year)}! Обязательно поздравь! Номер: {b.Phone}");
                        birthdaysToday++;
                    }

                    if (b.Date.DayOfYear < DateTime.Today.DayOfYear + 1)  //если ДР уже прошел
                        birthYear = yearNow + 1;
                    else  //если ДР еще будет
                        birthYear = yearNow; 

                    try //для обычных ДР
                    {
                        currentBirthday = new(birthYear, b.Date.Month, b.Date.Day);
                    }
                    catch (ArgumentOutOfRangeException) //если год високосный и ДР 29 февраля
                    {
                        currentBirthday = new(birthYear, b.Date.Month, b.Date.Day - 1);
                    }

                    int countOfDays = (currentBirthday - DateTime.Now).Days;
                    SortBirthdays sb = new(b.Name, b.Date, b.Phone, countOfDays + 1, currentBirthday.Year);
                    unsortedList.Add(sb);
                }

                if (birthdaysToday == 0) Console.WriteLine("\tПока ни у кого"); //если сегодня ДР никто не празднует
                var sortedList = unsortedList.OrderBy(si => si.InDays).ToList();
                int count;

                Console.WriteLine("\n***Ближайшие дни рождения в этом году***");
                
                count = 1;
                for (int i = 0; i < sortedList.Count; i++) //выводит ДР, который будет в этом году (из отсортированного списка)
                {
                    if (sortedList[i].InDays == 0) continue; //если ДР сегодня - пропуск
                    if (sortedList[i].Year == yearNow) //условие для вывода (если год текущего ДР совпадает с текущим годом)
                    {
                        string date = sortedList[i].Birthdate.ToLongDateString();
                        date = date.Remove(date.Length - 8); //дата словами
                        string event_info = sortedList[i].InDays == 1 ? "Завтра " : $"Через {sortedList[i].InDays} {RusWord(sortedList[i].InDays, false)} "; //сколько осталось до ДР
                        event_info += $"исполнится {yearNow - sortedList[i].Birthdate.Year} {RusWord(yearNow - sortedList[i].Birthdate.Year)}";
                        Console.WriteLine("\t{0, 2}){1, 15}{2, 15}{3, 15}{4, 40}", count, sortedList[i].Name, date, sortedList[i].Phone, event_info); //вывод с форматом
                        count++;
                    }
                }

                Console.WriteLine("\n***Ближайшие дни рождения в следующем году***");

                count = 1;
                for (int i = 0; i < sortedList.Count; i++) //выводит ДР, который будет в следующем году (из отсортированного списка)
                {
                    if (sortedList[i].Year != yearNow) //условие для вывода (если год текущего ДР не совпадает с текущим годом)
                    {
                        string date = sortedList[i].Birthdate.ToLongDateString();
                        date = date.Remove(date.Length - 8); //дата словами
                        string event_info = $"Через {sortedList[i].InDays} {RusWord(sortedList[i].InDays, false)} "; //сколько осталось до ДР
                        event_info += $"исполнится {yearNow + 1 - sortedList[i].Birthdate.Year} {RusWord(yearNow + 1 - sortedList[i].Birthdate.Year)}";
                        Console.WriteLine("\t{0, 2}){1, 15}{2, 15}{3, 15}{4, 40}", count, sortedList[i].Name, date, sortedList[i].Phone, event_info); //вывод с форматом
                        count++;
                    }
                }
            }

            ShowHelp(); //вывод списка команд

            while (true) //бесконечный цикл для чтения команд
            {
                Console.Write("\nКоманда -→ ");
                string? command = Console.ReadLine();

                if (command == null) //если ничего не введено
                    continue;

                else if (command == "help") //список команд
                    ShowHelp();

                else if (command == "res") //перезапуск (очистка консоли и вызов main)
                {
                    Console.Clear();
                    Main(args);
                }

                else if (command == "show") //список ДР
                    ShowBirthdays();

                else if (command == "add" || command == "upd") //если нужно добавить или изменить ДР
                {
                    if (command == "add") Console.WriteLine("--Добавление нового ДР--");
                    else Console.WriteLine("--Изменение существующего ДР--");
                    Console.WriteLine("(-→ cancel - для отмены)"); //чтобы отменить действие требуется ввести cancel

                    Birthday? birthday = null;
                    if (command == "upd") //если команда изменения - спрашивается ID
                    {
                        string? id;
                        do
                        {
                            Console.Write("\tВведите ID необходимого ДР: ");
                            id = Console.ReadLine();
                            birthday = CheckID(id); //получение ДР по его ID
                            if (birthday != null) break;
                        } while (id != "cancel");
                        if (id == "cancel") continue;
                    }

                    string? name; //ввод имени, при условии что его длина не длиннее 15
                    do
                    {
                        Console.Write("\tИмя: ");
                        name = Console.ReadLine();
                        if (name.Length <= 15) break;
                        else Console.WriteLine("Имя слишком длинное");
                    } while (name != "cancel");
                    if (name == "cancel") continue;

                    string? date;
                    int[]? dateArray = null; //ввод даты с проверкой корректного формата
                    do
                    {
                        Console.Write("\tДата рождения: ");
                        date = Console.ReadLine();
                        dateArray = CheckDate(date);
                        if (dateArray != null) break;
                    } while (date != "cancel");
                    if (date == "cancel") continue;

                    Console.Write("\tНомер телефона: "); //ввод телефона
                    string? phone = Console.ReadLine();
                    if (phone == "cancel") continue;

                    if (command == "add") AddBirthday(name, dateArray!, phone);
                    else UpdBirthday(birthday!, name, dateArray, phone);
                }

                else if (command == "del") //если нужно удалить ДР
                {
                    Console.WriteLine("--Удаление существующего ДР--");
                    Console.WriteLine("(-→ cancel - для отмены)");
                    string? id;
                    Birthday? birthday;
                    do
                    {
                        Console.Write("\tВведите ID необходимого ДР: ");
                        id = Console.ReadLine();
                        birthday = CheckID(id); //получение ДР по его ID
                        if (birthday != null) break;
                    } while (id != "cancel");
                    if (id == "cancel") continue;

                    string? confirm;
                    Console.Write("\tПодтвердите удаление (введите \"y\"): ");
                    confirm = Console.ReadLine();

                    if (confirm == "y") DelBirthday(birthday!); //если подтвердили  - удаляем
                }
                
                else Console.WriteLine("--Команда не распознана--");
            }
        }

        static void ShowHelp() //вывод списка команд
        {
            Console.WriteLine("\n--Команды для работы с программой--\n-→ help - Показать список команд");
            Console.WriteLine("-→ res - Перезапуск программы\n-→ show - Отобразить все ДР");
            Console.WriteLine("-→ add - Добавление нового ДР\n-→ upd - Изменение существующего ДР");
            Console.WriteLine("-→ del - Удаление существующего ДР");
        }
        static void ShowBirthdays() //вывод списка ДР
        {
            AppContext db = new();
            var birthdays = db.Birthdays.ToList();
            Console.WriteLine("\nСписок всех дней рождения:");
            foreach (Birthday b in birthdays)
                Console.WriteLine($"{b.Id}. {b.Name}, {b.Date.ToShortDateString()}, {b.Phone}");
        }
        static void AddBirthday(string name, int[] date, string? phone) //Добавление нового ДР
        {
            AppContext db = new();
            Birthday newbirthday = new() { Name = name, Date = new DateTime(date[0], date[1], date[2]), Phone = phone };
            db.Birthdays.Add(newbirthday);
            db.SaveChanges();

            Console.WriteLine("--ДР успешно добавлен!--\n");
        }
        static void UpdBirthday(Birthday birthday, string? name, int[]? date, string? phone) //Изменение ДР
        {
            AppContext db = new();
            birthday.Name = name;
            birthday.Date = new DateTime(date[0], date[1], date[2]);
            birthday.Phone = phone;
            db.Birthdays.Update(birthday);
            db.SaveChanges();

            Console.WriteLine($"--ДР ({birthday.Id}) успешно обновлен!--\n");
        }

        static void DelBirthday(Birthday birthday) //Удаление ДР
        {
            AppContext db = new();
            db.Birthdays.Remove(birthday);
            db.SaveChanges();

            Console.WriteLine($"--ДР ({birthday.Id}) успешно удален!--\n");
        }

        static string RusWord(int number, bool IsYear = true) //получение нужных окончаний слов по числу
        {
            string str = number.ToString();
            if (number >= 11 && number <= 20) 
                return IsYear ? "лет" : "дней";
            if (str.EndsWith('0') || str.EndsWith('5') || str.EndsWith('6') || str.EndsWith('7') || str.EndsWith('8') || str.EndsWith('9'))
                return IsYear ? "лет" : "дней";
            if (str.EndsWith('2') || str.EndsWith('3') || str.EndsWith('4'))
                return IsYear ? "года" : "дня";
            if (str.EndsWith('1'))
                return IsYear ? "год" : "день";
            return str;
        }

        static Birthday? CheckID(string? inputID) //проверка ДР по ID для изменения и удаления, и последующий вывод
        {
            Birthday? birthday = null;
            int id;
            try
            {
                id = int.Parse(inputID!);
                using AppContext db = new();
                Birthday? birthdays = db.Birthdays.FirstOrDefault();
                var birthdayList = db.Birthdays.ToList();
                foreach (var b in birthdayList)
                    if (b.Id == id) birthday = b;

                Console.WriteLine($"{birthday.Id}. {birthday.Name}, {birthday.Date.ToShortDateString()}, {birthday.Phone}");
            }
            catch
            {
                Console.WriteLine("--ДР с таким ID нет--");
            }
            return birthday;
        }

        static int[]? CheckDate(string? date) //проверка введенной даты
        {
            int[]? dateArray = null;
            try
            {
                dateArray = new int[3];
                dateArray[0] = int.Parse(date.Substring(6, 4));
                dateArray[1] = int.Parse(date.Substring(3, 2));
                dateArray[2] = int.Parse(date[..2]);
                var dt = new DateTime(dateArray[0], dateArray[1], dateArray[2]);
            }
            catch
            {
                Console.WriteLine("--Введена некорректная дата, попробуйте снова--");
                dateArray = null;
            }
            return dateArray;
        }
    }
}