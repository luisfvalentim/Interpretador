int fatorial(int n) {
    if (n <= 1) {
        return 1;
    }
    return n * fatorial(n - 1);
}

void main() {
    int resultado = fatorial(5);
    printf("Fatorial de 5: %d\n", resultado);
}
