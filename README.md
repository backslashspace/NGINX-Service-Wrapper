A small service wrapper for nginx on Windows

## What it does:
- Checks if nginx.exe is in the same directory as the service
- Starts nginx
- Runs GC and suspends itself
- Stop or shutdown Notifications from the service manager resume the wrapper
- Signal `-s quit` to nginx
- Exit

This also allows nginx to be reloaded by external programs while running

Note: reload signal processes must be started as the same user as the main process

---

## How to use:
1. Extract the wrapper into your nginx directory
2. Register the wrapper as a service
3. [Optionally] Configure the service to not use the Local System Account  