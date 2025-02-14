void main() {
    // Converter String para Float (atof)
    char conversao1[] = "3.14159";
    float num1 = atof(conversao1);  // Converte string para float

    printf("\n");
    printf("Converter String para Float");
    printf("String: %s\n", conversao1);
    printf("Float: %f\n", num1);

    //Converter String para Inteiro (atoi)
    char conversao2[] = "1234";
    int num2 = atoi(conversao2);  // Converte string para inteiro

    printf("\n");
    printf("Converter String para Inteiro");
    printf("String: %s\n", conversao2);
    printf("Inteiro: %d\n", num2);

    //Converter Inteiro para String (itoa)
    int conversao3 = 5678;
    string str1;  
    str1 = itoa(conversao3);  // Converte inteiro para string

    printf("\n");
    printf("Converter Inteiro para String");
    printf("Inteiro: %d\n", conversao3);
    printf("String: %s\n", str1);

    //Converter Float para String (ftoa)
    float conversao4 = 2.71;
    string str2;
    str2 = ftoa(conversao4, 3);  // Converte float para string com 3 casas decimais

    printf("\n");
    printf("Converter Float para String")/
    printf("Float: %f\n", conversao4);
    printf("String: %s\n", str2);
}
