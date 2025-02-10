int x = 10;
int y = 5;
int z = 10;

if (x == z) {
    printf("x é igual a z\n");
}

if (x != y) {
    printf("x é diferente de y\n");
}

if (y == z) {
    printf("y é igual a z (isso não será impresso)\n");
} else {
    printf("y não é igual a z\n");
}
