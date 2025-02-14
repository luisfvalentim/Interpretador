void main()
{
    int valor; // Erro: Usada antes da inicialização
    printf("Valor: %d\n", valor);

    valor = 3; //Correto: Agora está inicializada
    printf("Valor inicializado: %d\n", valor);
}