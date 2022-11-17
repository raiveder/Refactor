﻿namespace Refactor
{
    internal class Program
    {
        private static int[,] _matr;
        private static int[] _mConst;
        private static int[] _nConst;

        static void Main(string[] args)
        {
            ShowMenu();
        }

        /// <summary>
        /// Показ меню пользователю
        /// </summary>
        private static void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("1 - Ввести данные вручную.");
                Console.WriteLine("2 - Заполнить данные автоматически (по условию задачи).");
                Console.WriteLine("3 - Очистить экран.");
                Console.WriteLine("4 - Завершить работу.");
                Console.Write("Выберите действие: ");

                if (int.TryParse(Console.ReadLine(), out int result) && result > 0 && result < 5)
                {
                    switch (result)
                    {
                        case 1:
                            Console.Clear();
                            Vvod();
                            Calculation();
                            break;
                        case 2:
                            Console.Clear();
                            Fill();
                            Calculation();
                            break;
                        case 3:
                            Console.Clear();
                            break;
                        case 4:
                            Environment.Exit(0);
                            break;

                            // Блок default отсутствует, так как обработка ввода находится перед переходом к конструкции switch\case
                    }
                }
                else
                {
                    Console.WriteLine("Введите действие корректно!");
                }
            }
        }

        /// <summary>
        /// Определение вырожденности
        /// </summary>
        /// <param name="divisionArray">Распределение</param>
        /// <param name="m">Поставщики</param>
        /// <param name="n">Потребители</param>
        private static void Degenerate(bool[,] divisionArray, int m, int n)
        {
            int kol = 0;
            foreach (bool item in divisionArray)
            {
                if (item)
                {
                    kol++;
                }
            }

            if (m + n - 1 != kol)
            {
                int min = 16;
                int iMin = 0;
                int jMin = 0;
                for (int i = 0; i < _mConst.Length; i++)
                {
                    for (int j = 0; j < _nConst.Length; j++)
                    {
                        if (!divisionArray[i, j])
                        {
                            if (_matr[i, j] < min)
                            {
                                min = _matr[i, j];
                                iMin = i;
                                jMin = j;
                            }
                        }
                    }
                }
                divisionArray[iMin, jMin] = true;
            }
        }

        /// <summary>
        /// Заполнение ячеек цикла перераспределения
        /// </summary>
        /// <param name="divisionArrayBool">Информация о распределении</param>
        /// <param name="iMax">Индекс i максимальной дельты</param>
        /// <param name="jMax">Индекс j максимальной дельты</param>
        /// <returns>Строковый массив с индексами ячеек цикла перераспределения</returns>
        private static string[] Cycle(bool[,] divisionArrayBool, int iMax, int jMax)
        {
            Console.WriteLine("Введите индексы ячеек, через которые будет проходить цикл, без разделителей.\nНапример: 13 - ячейка [1;3].");
            Console.WriteLine("Цикл можно проводить только через ячейки, в которых находятся единицы.");
            Console.WriteLine("Для завершения ввода введите 0.");
            Console.WriteLine("Цикл начинается из ячейки [{0},{1}]. Введите следующие ячейки.", iMax + 1, jMax + 1);

            string[] array = new string[1];
            array[0] = Convert.ToString(iMax) + Convert.ToString(jMax);
            int i = 1;
            string check = "0";

            while (true)
            {
                while (true)
                {
                    if (i % 2 == 0)
                    {
                        Console.Write("Введите ячейку цикла (+х): ");
                    }
                    else
                    {
                        Console.Write("Введите ячейку цикла (-х): ");
                    }

                    check = Console.ReadLine();
                    if (int.TryParse(check, out int result) && result > 10 && result < 35 || check == "0")
                    {
                        if (check == "0")
                        {
                            Console.Clear();
                            return array;
                        }
                        if (divisionArrayBool[Convert.ToInt32(check.Substring(0, 1)) - 1, Convert.ToInt32(check.Substring(1, 1)) - 1])
                        {
                            Array.Resize(ref array, array.Length + 1);
                            array[i] = check;
                            i++;
                            break;
                        }
                    }
                    Console.WriteLine("Ошибка. Введите ячейку корректно!");
                }
            }
        }

        /// <summary>
        /// Определение вида распределения
        /// </summary>
        /// <param name="divisionArray">Распределение</param>
        /// <param name="divisionArrayBool">Информация о распределении (есть ли связь между поставщиком и потребителем)</param>
        /// <returns>Оптимальное распределение (true) или нет (false)</returns>
        private static bool GetTypeDivision(int[,] divisionArray, bool[,] divisionArrayBool)
        {
            int[] v = new int[_mConst.Length];
            int[] u = new int[_nConst.Length];
            CalcPotenzials(v, u, divisionArrayBool);

            int[,] delta = GetDelta(divisionArrayBool, v, u);

            bool check = true;
            foreach (int item in delta)
            {
                if (item > 0)
                {
                    check = false;
                    break;
                }
            }

            if (check)
            {
                return true;
            }

            ShowCycle(delta, divisionArrayBool, out int iMax, out int jMax);
            string[] cycleArray = Cycle(divisionArrayBool, iMax, jMax);

            Division(cycleArray, divisionArray, divisionArrayBool);

            return false;
        }

        /// <summary>
        /// Расчёт потенциалов
        /// </summary>
        /// <param name="v">Потенциалы поставщиков</param>
        /// <param name="u">Потенциалы потребителей</param>
        /// <param name="divisionArrayBool">Информация о распределении</param>
        private static void CalcPotenzials(int[] v, int[] u, bool[,] divisionArrayBool)
        {
            bool[] vBool = new bool[_mConst.Length];
            bool[] uBool = new bool[_nConst.Length];
            uBool[0] = true;

            for (int i = 0; i < _mConst.Length; i++)
            {
                if (divisionArrayBool[i, 0])
                {
                    v[i] = _matr[i, 0];
                    vBool[i] = true;
                }
            }

            bool checkV = true;
            bool checkU = true;
            while (checkV || checkU)
            {
                for (int i = 0; i < _mConst.Length; i++)
                {
                    for (int j = 1; j < _nConst.Length; j++)
                    {
                        if (divisionArrayBool[i, j])
                        {
                            if (uBool[j])
                            {
                                v[i] = _matr[i, j] - u[j];
                                vBool[i] = true;
                            }

                            if (vBool[i])
                            {
                                u[j] = _matr[i, j] - v[i];
                                uBool[j] = true;
                            }
                        }
                    }
                }

                checkV = false;
                foreach (bool item in vBool)
                {
                    if (item == false)
                    {
                        checkV = true;
                        break;
                    }
                }

                checkU = false;
                foreach (bool item in uBool)
                {
                    if (item == false)
                    {
                        checkU = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Перераспределение
        /// </summary>
        /// <param name="cycleArray">Ячейки цикла</param>
        /// <param name="divisionArray">Массив с распределением</param>
        /// <param name="divisionArrayBool">Информация о распределении</param>
        private static void Division(string[] cycleArray, int[,] divisionArray, bool[,] divisionArrayBool)
        {
            int iMin = Convert.ToInt32(cycleArray[1].Substring(0, 1)) - 1;
            int jMin = Convert.ToInt32(cycleArray[1].Substring(1, 1)) - 1;
            int min = divisionArray[iMin, jMin];

            for (int i = 3; i < cycleArray.Length; i += 2)
            {
                iMin = Convert.ToInt32(cycleArray[i].Substring(0, 1)) - 1;
                jMin = Convert.ToInt32(cycleArray[i].Substring(1, 1)) - 1;
                if (divisionArray[iMin, jMin] < min)
                {
                    min = divisionArray[iMin, jMin];
                }
            }

            int indexI = Convert.ToInt32(cycleArray[0].Substring(0, 1));
            int jndexJ = Convert.ToInt32(cycleArray[0].Substring(1, 1));
            divisionArray[indexI, jndexJ] += min;

            if (divisionArray[indexI, jndexJ] > 0)
            {
                divisionArrayBool[indexI, jndexJ] = true;
            }

            for (int c = 1; c < cycleArray.Length; c++)
            {
                int i = Convert.ToInt32(cycleArray[c].Substring(0, 1)) - 1;
                int j = Convert.ToInt32(cycleArray[c].Substring(1, 1)) - 1;
                if (c % 2 == 0)
                {
                    divisionArray[i, j] += min;
                    if (divisionArray[i, j] > 0)
                    {
                        divisionArrayBool[i, j] = true;
                    }
                }
                else
                {
                    divisionArray[i, j] -= min;
                    if (divisionArray[i, j] == 0)
                    {
                        divisionArrayBool[i, j] = false;
                    }
                }
            }
        }

        /// <summary>
        /// Отображение матрицы для выбора цикла
        /// </summary>
        /// <param name="delta">Дельта</param>
        /// <param name="divisionArrayBool">Информация о распределении</param>
        /// <param name="iMax">Индекс i максимального элемента массива delta</param>
        /// <param name="jMax">Индекс j максимального элемента массива delta</param>
        private static void ShowCycle(int[,] delta, bool[,] divisionArrayBool, out int iMax, out int jMax)
        {
            int max = delta[0, 0];
            iMax = 0;
            jMax = 0;

            for (int i = 0; i < _mConst.Length; i++)
            {
                for (int j = 0; j < _nConst.Length; j++)
                {
                    if (delta[i, j] > max)
                    {
                        max = delta[i, j];
                        iMax = i;
                        jMax = j;
                    }
                }
            }

            for (int i = 0; i < _mConst.Length; i++)
            {
                for (int j = 0; j < _nConst.Length; j++)
                {
                    if (divisionArrayBool[i, j])
                    {
                        Console.Write(1 + "\t");
                    }
                    else
                    {
                        Console.Write(0 + "\t");
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Расчёт дельты
        /// </summary>
        /// <param name="divisionArrayBool">Информация о распределении (есть ли связь между поставщиком и потребителем)</param>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns>Дельта</returns>
        private static int[,] GetDelta(bool[,] divisionArrayBool, int[] v, int[] u)
        {
            int[,] delta = new int[_mConst.Length, _nConst.Length];
            for (int i = 0; i < _mConst.Length; i++)
            {
                for (int j = 0; j < _nConst.Length; j++)
                {
                    if (!divisionArrayBool[i, j])
                    {
                        delta[i, j] = v[i] + u[j] - _matr[i, j];
                    }
                }
            }

            return delta;
        }

        /// <summary>
        /// Расчёт целевой функции
        /// </summary>
        private static void Calculation()
        {
            int[] m = new int[_mConst.Length];
            int[] n = new int[_nConst.Length];
            int[,] firstDivision = new int[m.Length, n.Length];

            Array.Copy(_matr, firstDivision, _matr.Length);
            Array.Copy(_mConst, m, _mConst.Length);
            Array.Copy(_nConst, n, _nConst.Length);

            bool[,] divisionArrayBool = new bool[m.Length, n.Length];
            int[,] divisionArray = new int[m.Length, n.Length];
            int func = 0;
            string funcS = null;
            int sum = 1;

            while (sum != 0)
            {
                int min = 16;
                int iMin = 0;
                int jMin = 0;

                for (int i = 0; i < m.Length; i++)
                {
                    for (int j = 0; j < n.Length; j++)
                    {
                        if (firstDivision[i, j] < min && firstDivision[i, j] > 0)
                        {
                            min = firstDivision[i, j];
                            iMin = i;
                            jMin = j;
                        }
                    }
                }

                firstDivision[iMin, jMin] = 0;

                if (m[iMin] > 0 && n[jMin] > 0)
                {
                    if (n[jMin] < m[iMin])
                    {
                        func += _matr[iMin, jMin] * n[jMin];
                        funcS += _matr[iMin, jMin] + "*" + n[jMin] + " + ";
                        divisionArray[iMin, jMin] = n[jMin];
                        divisionArrayBool[iMin, jMin] = true;
                        m[iMin] -= n[jMin];
                        n[jMin] = 0;
                    }
                    else
                    {
                        func += _matr[iMin, jMin] * m[iMin];
                        funcS += _matr[iMin, jMin] + "*" + m[iMin] + " + ";
                        divisionArray[iMin, jMin] = m[iMin];
                        divisionArrayBool[iMin, jMin] = true;
                        n[jMin] -= m[iMin];
                        m[iMin] = 0;
                    }
                }

                sum = 0;
                foreach (int item in m)
                {
                    sum += item;
                }
            }

            Degenerate(divisionArrayBool, m.Length, n.Length);

            bool check = GetTypeDivision(divisionArray, divisionArrayBool);

            if (!check)
            {
                while (check != true)
                {
                    funcS = "";
                    func = 0;

                    Array.Copy(_mConst, m, _mConst.Length);
                    Array.Copy(_nConst, n, _nConst.Length);

                    for (int i = 0; i < m.Length; i++)
                    {
                        for (int j = 0; j < n.Length; j++)
                        {
                            if (divisionArray[i, j] > 0)
                            {
                                if (n[j] < m[i])
                                {
                                    func += _matr[i, j] * divisionArray[i, j];
                                    funcS += _matr[i, j] + "*" + divisionArray[i, j] + " + ";
                                }
                                else
                                {
                                    func += _matr[i, j] * divisionArray[i, j];
                                    funcS += _matr[i, j] + "*" + divisionArray[i, j] + " + ";
                                }
                            }
                        }
                    }

                    check = GetTypeDivision(divisionArray, divisionArrayBool);
                }
            }

            funcS = "Ответ: Fопт = " + funcS.Substring(0, funcS.Length - 3) + " = " + func + " у.д.е.";
            Console.WriteLine(funcS + "\n");

            ShowContracts(divisionArray, m.Length, n.Length);
        }

        /// <summary>
        /// Вывод порядка заклюычения договоров
        /// </summary>
        /// <param name="divisionArray">Распределение</param>
        /// <param name="countM">Количество поставщиков</param>
        /// <param name="countN">Количество потребителей</param>
        private static void ShowContracts(int[,] divisionArray, int countM, int countN)
        {
            Console.WriteLine("\t\tЗаключение договоров");
            for (int i = 0; i < countM; i++)
            {
                for (int j = 0; j < countN; j++)
                {
                    if (divisionArray[i, j] > 0)
                    {
                        Console.WriteLine("{0}-й поставщик с {1}-м потребителем на {2} ед. продукции", i + 1, j + 1, divisionArray[i, j]);
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Заполнение исходных данных по условию задачи
        /// </summary>
        private static void Fill()
        {
            _matr = new int[3, 4];
            _matr[0, 0] = 9;
            _matr[0, 1] = 5;
            _matr[0, 2] = 3;
            _matr[0, 3] = 10;
            _matr[1, 0] = 6;
            _matr[1, 1] = 3;
            _matr[1, 2] = 8;
            _matr[1, 3] = 2;
            _matr[2, 0] = 3;
            _matr[2, 1] = 8;
            _matr[2, 2] = 4;
            _matr[2, 3] = 7;

            _mConst = new int[3];
            _mConst[0] = 25;
            _mConst[1] = 55;
            _mConst[2] = 22;

            _nConst = new int[4];
            _nConst[0] = 45;
            _nConst[1] = 15;
            _nConst[2] = 22;
            _nConst[3] = 20;
        }

        /// <summary>
        /// Ввод исходных данных вручную
        /// </summary>
        private static void Vvod() //Отдельные методы
        {
            int m = InputM();
            int n = InputN();

            _mConst = new int[m];
            _nConst = new int[n];
            _matr = new int[m, n];

            while (true)
            {
                InputMValue();
                InputNValue();

                int sumM = 0;
                foreach (int item in _mConst)
                {
                    sumM += item;
                }

                int sumN = 0;
                foreach (int item in _nConst)
                {
                    sumN += item;
                }

                if (sumM == sumN)
                {
                    Console.Clear();
                    break;
                }
                Console.WriteLine("Ошибка. Суммарные мощности поставщиков и спросов потребителей не равны!");
            }

            CreateMatrix();

            Console.Clear();
        }

        private static int InputM()
        {
            while (true)
            {
                Console.Write("Введите количество поставщиков: ");
                if (int.TryParse(Console.ReadLine(), out int m) && m > 1 && m < 11)
                {
                    return m;
                }
                Console.WriteLine("Ошибка. Введите значение корректно!");
            }
        }

        private static int InputN()
        {
            while (true)
            {
                Console.Write("Введите количество потребителей: ");
                if (int.TryParse(Console.ReadLine(), out int n) && n > 1 && n < 11)
                {
                    return n;
                }
                Console.WriteLine("Ошибка. Введите значение корректно!");
            }
        }

        private static void CreateMatrix()
        {
            Console.WriteLine("Заполнение матрицы затрат на перевозку.");

            for (int i = 0; i < _mConst.Length; i++)
            {
                for (int j = 0; j < _nConst.Length; j++)
                {
                    while (true)
                    {
                        Console.Write("Введите [{0},{1}] элемент матрицы: ", i + 1, j + 1);
                        if (int.TryParse(Console.ReadLine(), out int result) && result > 0 && result < 16)
                        {
                            _matr[i, j] = result;
                            break;
                        }
                        Console.WriteLine("Ошибка. Введите значение корректно!");
                    }
                }
            }
        }

        private static void InputMValue()
        {
            for (int i = 0; i < _mConst.Length; i++)
            {
                while (true)
                {
                    Console.Write("Введите мощность {0}-го поставщика: ", i + 1);
                    if (int.TryParse(Console.ReadLine(), out int result) && result > 1 && result < 151)
                    {
                        _mConst[i] = result;
                        break;
                    }
                    Console.WriteLine("Ошибка. Введите значение корректно!");
                }
            }
        }

        private static void InputNValue()
        {
            for (int i = 0; i < _nConst.Length; i++)
            {
                while (true)
                {
                    Console.Write("Введите спрос {0}-го потребителя: ", i + 1);
                    if (int.TryParse(Console.ReadLine(), out int result) && result > 1 && result < 151)
                    {
                        _nConst[i] = result;
                        break;
                    }
                    Console.WriteLine("Ошибка. Введите значение корректно!");
                }
            }
        }
    }
}