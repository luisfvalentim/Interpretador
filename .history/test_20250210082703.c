void main()
{
    int opcao = 2;

switch (opcao) {
    case 1:
        printf("Opção 1 escolhida\n");
        break;
    case 2:
        printf("Opção 2 escolhida\n");  // ✅ Saída esperada: "Opção 2 escolhida"
        break;
    case 3:
        printf("Opção 3 escolhida\n");
        break;
    default:
        printf("Opção inválida\n");
}

}
