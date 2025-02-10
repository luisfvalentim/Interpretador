int x = 10;
int y = 5;
int z = 0;

if (x > y && y > z) {
    printf("x é maior que y E y é maior que z\n");
}

if (x < y && y > z) {
    printf("Isso não será impresso porque x < y é falso\n");
} else {
    printf("O segundo if falhou, então caímos no else\n");
}

if (x < y || y > z) {
    printf("x < y é falso, mas y > z é verdadeiro, então OR funciona\n");
}

if (x < y || z) {
    printf("Isso não será impresso porque ambas as condições são falsas\n");
} else {
    printf("Como ambas as condições eram falsas, o else foi executado\n");
}

