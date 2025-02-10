int n = 7;
int i = 2;
int primo = 1;

while (i < n) {
    if (n % i == 0) {
        primo = 0;
        break;
    }
    i = i + 1;
}

if (primo == 1) {
    printf("%d é primo\n", n);
} else {
    printf("%d não é primo\n", n);
}
