using OrderProcessing.Api.Domain;

namespace OrderProcessing.Api.States;

public class InvalidOrderTransitionException : Exception
{
    public InvalidOrderTransitionException(string stateName, string action)
        : base($"Nu pot executa '{action}' în starea '{stateName}'") { }
}

public interface IOrderState
{
    string Name { get; }
    void Pay(Order order);
    void Process(Order order);
    void Ship(Order order);
    void Deliver(Order order);
    void Cancel(Order order);
}

public class PendingState : IOrderState
{
    public string Name => "Pending";
    public void Pay(Order order)     => order.Transition(new ConfirmedState());
    public void Process(Order order) => throw new InvalidOrderTransitionException(Name, "Process");
    public void Ship(Order order)    => throw new InvalidOrderTransitionException(Name, "Ship");
    public void Deliver(Order order) => throw new InvalidOrderTransitionException(Name, "Deliver");
    public void Cancel(Order order)  => order.Transition(new CancelledState());
}

public class ConfirmedState : IOrderState
{
    public string Name => "Confirmed";
    public void Pay(Order order)     => throw new InvalidOrderTransitionException(Name, "Pay");
    public void Process(Order order) => order.Transition(new ProcessingState());
    public void Ship(Order order)    => throw new InvalidOrderTransitionException(Name, "Ship");
    public void Deliver(Order order) => throw new InvalidOrderTransitionException(Name, "Deliver");
    public void Cancel(Order order)  => order.Transition(new CancelledState());
}

public class ProcessingState : IOrderState
{
    public string Name => "Processing";
    public void Pay(Order order)     => throw new InvalidOrderTransitionException(Name, "Pay");
    public void Process(Order order) => throw new InvalidOrderTransitionException(Name, "Process");
    public void Ship(Order order)    => order.Transition(new ShippedState());
    public void Deliver(Order order) => throw new InvalidOrderTransitionException(Name, "Deliver");
    public void Cancel(Order order)  => order.Transition(new CancelledState());
}

public class ShippedState : IOrderState
{
    public string Name => "Shipped";
    public void Pay(Order order)     => throw new InvalidOrderTransitionException(Name, "Pay");
    public void Process(Order order) => throw new InvalidOrderTransitionException(Name, "Process");
    public void Ship(Order order)    => throw new InvalidOrderTransitionException(Name, "Ship");
    public void Deliver(Order order) => order.Transition(new DeliveredState());
    public void Cancel(Order order)  => throw new InvalidOrderTransitionException(Name, "Cancel");
}

public class DeliveredState : IOrderState
{
    public string Name => "Delivered";
    public void Pay(Order order)     => throw new InvalidOrderTransitionException(Name, "Pay");
    public void Process(Order order) => throw new InvalidOrderTransitionException(Name, "Process");
    public void Ship(Order order)    => throw new InvalidOrderTransitionException(Name, "Ship");
    public void Deliver(Order order) => throw new InvalidOrderTransitionException(Name, "Deliver");
    public void Cancel(Order order)  => throw new InvalidOrderTransitionException(Name, "Cancel");
}

public class CancelledState : IOrderState
{
    public string Name => "Cancelled";
    public void Pay(Order order)     => throw new InvalidOrderTransitionException(Name, "Pay");
    public void Process(Order order) => throw new InvalidOrderTransitionException(Name, "Process");
    public void Ship(Order order)    => throw new InvalidOrderTransitionException(Name, "Ship");
    public void Deliver(Order order) => throw new InvalidOrderTransitionException(Name, "Deliver");
    public void Cancel(Order order)  => throw new InvalidOrderTransitionException(Name, "Cancel");
}
