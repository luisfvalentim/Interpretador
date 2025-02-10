void main() {
    int idade;  
float altura;  
char inicial;  
string nome;  

// ❌ Tentando usar variáveis antes da inicialização (devem gerar erro)
printf("Idade: %d\n", idade);   // Erro: Variável 'idade' usada antes da inicialização.
printf("Altura: %.2f\n", altura); // Erro: Variável 'altura' usada antes da inicialização.
printf("Inicial: %c\n", inicial); // Erro: Variável 'inicial' usada antes da inicialização.
printf("Nome: %s\n", nome);    // Erro: Variável 'nome' usada antes da inicialização.

// ✅ Inicializando corretamente as variáveis
idade = 25;
altura = 1.75;
inicial = 'A';
nome = "Carlos";

// ✅ Agora as variáveis podem ser usadas sem erro
printf("Idade: %d\n", idade);   // Saída: 25
printf("Altura: %.2f\n", altura); // Saída: 1.75
printf("Inicial: %c\n", inicial); // Saída: A
printf("Nome: %s\n", nome);    // Saída: Carlos
               // Erro: Inteiro atribuído a string
}
