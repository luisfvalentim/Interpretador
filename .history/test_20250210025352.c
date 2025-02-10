struct Pessoa {
    int idade;
    string nome;
};

Pessoa p;
p.idade = 25;
p.nome = "Carlos";

printf("Nome: %s, Idade: %d\n", p.nome, p.idade); // ✅ Deve imprimir: Nome: Carlos, Idade: 25
