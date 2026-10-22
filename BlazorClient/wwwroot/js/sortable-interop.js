window.sortableInterop = {
    _instances: new WeakMap(),

    create: function (element, dotNetRef, methodName) {
        if (!element) return;
        const instance = new Sortable(element, {
            animation: 150,
            handle: ".ordered-multiselect__handle",
            ghostClass: "ordered-multiselect__ghost",
            onEnd: function () {
                const ids = Array.from(element.children).map(c => c.dataset.id);
                dotNetRef.invokeMethodAsync(methodName, ids);
            }
        });
        window.sortableInterop._instances.set(element, instance);
    },

    refresh: function (element) {
        // SortableJS сам отслеживает изменения DOM, refresh не нужен,
        // но метод оставлен на случай, если понадобится `instance.sort(...)`.
    },

    destroy: function (element) {
        const instance = window.sortableInterop._instances.get(element);
        if (instance) {
            instance.destroy();
            window.sortableInterop._instances.delete(element);
        }
    }
};