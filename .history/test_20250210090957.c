int fatorial(int n) {
    if (n == 0) return 1; // Caso base
    return n * fatorial(n - 1); // Chamada recursiva
}

int main() {
    int num = 5;
    printf("O fatorial de %d é %d\n", num, fatorial(num));
    return 0;
}


