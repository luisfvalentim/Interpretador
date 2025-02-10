struct Pessoa {
    string nome;
    int idade;
};

Pessoa p;
p.nome = "Carlos";
p.idade = 25;
printf("Nome: %s, Idade: %d\n", p.nome, p.idade); // ✅ Deve imprimir "Carlos, 25"



