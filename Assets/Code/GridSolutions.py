
# - - -

print("\n\n ___ START ___ \n")


def generate_grid(n):
    return [[0 for i in range(n)] for i in range(n)]


def printGrid(grid):
    print("\n - - - ")
    for row in grid:
        print(row)
    print(" - - - ")

def placeTree(grid, x, y):
    grid[x][y] = 3

    for i in range(len(grid)):
        if (x != i ): grid[i][y] = 1
        if (y != i ): grid[x][i] = 1

    if x > 0 and y > 0:                         grid[x-1][y-1] = 1
    if x > 0 and y < len(grid) - 1:             grid[x-1][y+1] = 1
    if x < len(grid) - 1 and y > 0:             grid[x+1][y-1] = 1
    if x < len(grid) - 1 and y < len(grid) - 1: grid[x+1][y+1] = 1

def isGridFull(grid):
    for x in range(len(grid)):
        for y in range(len(grid[x])):
            if grid[x][y] == 0:
                return False
    return True

def isGridSolved(grid):
    count = len(grid)
    for x in range(len(grid)):
        for y in range(len(grid[x])):
            if grid[x][y] == 3:
                count -= 1
    return count == 0


def fillGrid(grid, x):
    for y in range(len(grid[x])):
        if grid[x][y] == 0:
            gCopy = [row[:] for row in grid]
            placeTree(gCopy, x, y)
            if isGridFull(gCopy) and isGridSolved(gCopy) and gridExists(gCopy) == False:
                storeGrid(gCopy)
            else:
                fillGrid(gCopy, x+1)
                

storage = {}

def storeGrid(grid):
    grid_tuple = tuple(tuple(row) for row in grid)
    storage[grid_tuple] = grid

def gridExists(grid):
    grid_tuple = tuple(tuple(row) for row in grid)
    return grid_tuple in storage

# - - -
# MAIN 
# - - -

gridSize = 5
grid = generate_grid(gridSize)

fillGrid(grid, 0)

printGrid(storage[list(storage.keys())[3]])

print("\n - Found ", len(storage), " Solutions ! ")

print("\n ___ END ___ \n")


# - - -