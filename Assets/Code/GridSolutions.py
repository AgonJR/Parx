
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


def FindAllSolutions(grid, x):
    if x >= len(grid):
        return
    for y in range(len(grid[x])):
        if grid[x][y] == 0:
            gCopy = [row[:] for row in grid]
            placeTree(gCopy, x, y)
            if isGridFull(gCopy) and isGridSolved(gCopy) and gridExists(gCopy) == False:
                storeGrid(gCopy)
            else:
                FindAllSolutions(gCopy, x+1)
                

storage = []

def convertGridToInt(grid):
    val = ""
    for x in range(len(grid)):
        for y in range(len(grid[x])):
            if grid[x][y] == 3:
                val += str(y)
    return val
    

def storeGrid(grid):
    val = convertGridToInt(grid)
    if val in storage:
        return
    storage.append(val)

def gridExists(grid):
    if storage.count == 0: return False
    return convertGridToInt(grid) in storage

def saveToFile(strings, fileName):
    with open(fileName, 'w') as file:
        for string in strings:
            file.write(string + '\n')


# - - -
# MAIN 
# - - -

gridSize = 8
grid = generate_grid(gridSize)

FindAllSolutions(grid, 0)

x = 0
for c in storage[9]:
    placeTree(grid, x, int(c))
    x += 1

printGrid(grid)

file = "gridSolutions_" + str(gridSize) + ".txt"
saveToFile(storage, file)

print("\n - Found ", len(storage), " Solutions ! ")

print("\n ___ END ___ \n")


# - - -