# Sprint 4 — Agenda inteligente

A disponibilidade é calculada no servidor. Um horário só aparece quando:

1. o profissional está ativo;
2. realiza o serviço selecionado;
3. possui jornada ativa naquele dia;
4. o atendimento termina antes do fim da jornada;
5. não se sobrepõe a outra reserva não cancelada;
6. está no futuro.

A verificação é repetida no momento da confirmação para reduzir conflitos quando dois usuários consultam o mesmo horário.
