int fatorial(int n) {
    if (n == 0) {
        return 1;
    }
    return n * fatorial(n - 1);
}

printf("Fatorial de 5: %d\n", fatorial(5));
