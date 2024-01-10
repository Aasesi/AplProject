// dllmain.cpp : Definiuje punkt wejścia dla aplikacji DLL.
#include "pch.h"

/*
BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

*/

// Jak to działa? - Dla kazdej wartosci red green blue ktora dostajemy z array uzywamy "if" by sprawdzic czy jest wieksza od thresholdu jesli tak to przyznaje "1" jesli nie to "0"
// Pozniej jesli suma trzech elementów jest rowna lub wieksza od 2 to znaczy ze element jest rowny 255 czyli Biały w innym przypadku jest czarny czyli 0
// Threshold Jest wpisywany samodzielnie w parametrze Wieęc jak dacie np 1 to bedzie caly czarny obraz.
extern "C" __declspec(dllexport) int* Add(int arr[], int size, int threshold) {
    int* changed_array = new int[size];
    int index = 0;
    for (int i = 0; i < size; i+=3)
    {
        int new_red = 0;
        int new_green = 0;
        int new_blue = 0;
        if (arr[i] > threshold)
        {
            new_red = 1;
        }
        if (arr[i + 1] > threshold)
        {
            new_green = 1;
        }
        if (arr[i + 2] > threshold)
        {
            new_blue = 1;
        }

        if (new_red + new_blue + new_green >= 2)
        {
            changed_array[index] = 255;
            changed_array[index+1] = 255;
            changed_array[index+2] = 255;
            index += 3;
        }
        else
        {
            changed_array[index] = 0;
            changed_array[index + 1] = 0;
            changed_array[index + 2] = 0;
            index += 3;
        }
    }
    return changed_array;
}
